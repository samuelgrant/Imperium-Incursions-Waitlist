import React, { Component } from 'react';
import { render } from 'react-dom';
import Alert from './Components/Alert';
import Waitlist from './Components/Waitlist';
import Glance from './Components/FleetGlance';
import { SidePanel, SideSection, SidePanelButton } from './Components/SidePanel';
import { BtnClose, BtnClear, BtnInvAll, BtnInvFaxes, Backseat, Boss, ExitCyno, ExitCyno_Add, Mumble, Status, Type } from './Components/FleetSettings';

const baseUri = "/fleets";

export default class Index extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fcOptions: null,
            fleetId: null
        }
    }

    componentDidMount() {
        this.setState({
            fleetId: $("#fleetManagement").data("fleetid")
        }, () => this.getFleetData());

        setInterval(() => this.getFleetData(), 1000 * 10);
    }

    isPublic() {
        return (this.state.fleet) ? this.state.fleet.isPublic : null;
    }

    getFleetData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/${this.state.fleetId}/data`
        }).done((result) => {
            this.setState({ fleet: result });
            this.getWaitlistData();
        }).fail((err) => {
            if (err.statusCode === 404)
                location.href = '/';
            console.error(`React/FleetManagement {FleetManagement@getFleetData} - Error getting fleet information`, err.responseText);
        });

        // Remove row highlights
        $("tr").removeClass("tr-danger")
            .removeClass("tr-success")
            .removeClass("tr-pending");
    }

    getWaitlistData() {
        $.ajax({
            type: 'get',
            url: `/api/v1/waitlist/pilots`
        }).done((result) => {
            this.setState({ waitingPilots: result });
            this.getFcSettings();
        }).fail((err) => {
            console.error(`React/FleetManagement {FleetManagement@getWaitlistData} - Error getting fleet information`, err.responseText);
        });
    }

    getFcSettings() {
        $.ajax({
            type: 'get',
            url: `/api/v1/options`
        }).done((settings) => {
            this.setState({ fcOptions: settings.fcOptions });
        }).fail((err) => {
            console.error(`React/FleetManagement {Index@getFcSettings} - Error getting the FC settings`, err.responseText);
        })
    }

    getFleetWings() {
        return this.state.fleet && this.state.fleet.wings ? this.state.fleet.wings : null;
    }

    getFleetSettings() {
        return this.state.fleet || null;
    }

    getWaitlist() {
        return this.state.waitingPilots || null;
    }

    getSettings() {
        return this.state.fcOptions || null;
    }

    hasFleetBoss() {
        return this.state.fleet && this.state.fleet.bossPilot;
    }

    getMembers() {
        return this.state.fleet && this.state.fleet.members ? this.state.fleet.members : null;
    }

    fleetCynos() {
        let members = this.getMembers();
        if (members == null)
            return null;

        let cynoArr = [];
        for (let i = 0; i < members.pilots.length; i++)
            if (members.pilots[i].isExitCyno)
                cynoArr.push(members.pilots[i]);

        return cynoArr;
    }

    render() {
        let fleetPrivate;
        if (!this.isPublic()) {
            fleetPrivate = (
                <Alert type="danger">
                    <span className="font-weight-bold">Fleet Not Listed: </span>
                    If no fleets are listed, the waitlist will show as offline.
                </Alert>
            );
        }

        let noFleetBoss;
        if (!this.hasFleetBoss()) {
            noFleetBoss = (
                <Alert type="danger">
                    <span className="font-weight-bold">No Fleet Boss: </span>
                    Until you set a new fleet boss all ESI functions will be disabled for this fleet.
                </Alert>
            );
        }

        let needCynos;
        if (!this.fleetCynos() || this.fleetCynos().length == 0) {
            needCynos = (
                <Alert type="danger">
                    <span className="font-weight-bold">
                        <i className="fas fa-exclamation-triangle"></i>
                         Fleet Cynos Needed:
                    </span>

                    You MUST have at least one exit cyno in this fleet!
                </Alert>
            )
        }

        let fleetSize;
        let fleetSizeClass;
        if (this.state.fleet && this.state.fleet.members) {
            let onGrid = this.state.fleet.members.onGrid;
            let max = this.state.fleet.members.max;

            fleetSize = `${this.state.fleet.members.onGrid} / ${this.state.fleet.members.max}`;

            if (onGrid > max) {
                fleetSizeClass = `danger blink`;
            } else if (onGrid < max - 5) {
                fleetSizeClass = `warning`;
            } else {
                fleetSizeClass = `white`;
            }
        }

        return (
            <div className="container">
                {fleetPrivate}
                {noFleetBoss}
                {needCynos}

                <div className="row">
                    <div className="col-lg-8 col-sm-12">
                        <Waitlist waitlist={this.getWaitlist()} wings={this.getFleetWings()} fleetId={this.state.fleetId || null} />
                    </div>

                    <div className="col-lg-4 col-sm-12">
                        <div className="text-center">
                            <SidePanelButton id="fleetSettings" title="Fleet Settings" />
                            <SidePanelButton id="fleetCynos" title="Fleet Cynos" />
                        </div>

                        <h4 className={`d-block text-center text-${fleetSizeClass} py-4`}>Fleet Size: {fleetSize}</h4>

                        <Glance members={this.getMembers()} />
                    </div>
                </div>


                <SidePanel id="fleetSettings" title="Fleet Settings">
                    <div className="row">
                        <Boss pilot={(this.state.fleet) ? this.state.fleet.bossPilot : null}
                            pilots={(this.getSettings()) ? this.getSettings().pilots : null}
                            u={this.getFleetData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Backseat account={(this.state.fleet) ? this.state.fleet.backseatAccount : null}
                            u={this.getFleetData.bind(this)}
                            fleetId={this.state.fleetId}
                            pilots={this.state.fcOptions ? this.state.fcOptions.pilots : null}/>

                        <Mumble channel={(this.getFleetSettings()) ? this.getFleetSettings().commChannel : null}
                            options={(this.getSettings()) ? this.getSettings().comms : null}
                            u={this.getFleetData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Type type={(this.getFleetSettings()) ? this.getFleetSettings().type : ""}
                            options={(this.getSettings()) ? this.getSettings().fleetTypes : null}
                            u={this.getFleetData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Status public={this.isPublic()}
                            u={this.getFleetData.bind(this)}
                            fleetId={this.state.fleetId} />
                    </div>

                    <hr />

                    <div className="row">
                        <BtnClose fleetId={this.state.fleetId} />
                        <BtnClear fleetId={this.state.fleetId} u={this.getFleetData.bind(this) }/>

                        <BtnInvAll fleetId={this.state.fleetId} />
                        <BtnInvFaxes fleetId={this.state.fleetId} />
                    </div>
                </SidePanel>
                
                    
                <SidePanel id="fleetCynos" title="Fleet Cynos">
                    <ExitCyno_Add
                        myPilots={this.state.fcOptions !=null ? this.state.fcOptions.pilots : null}
                        fleetPilots={this.state.fleet != null ? this.state.fleet.members.pilots : null}
                        u={this.getFleetData.bind(this)}
                        fleetId={this.state.fleetId} />

                    <hr />

                    <ExitCyno
                        cynos={this.fleetCynos()}
                        u={this.getFleetData.bind(this)}
                        fleetId={this.state.fleetId} />
                </SidePanel>
            </div>
        )
    }
}



if (document.getElementById('fleetManagement'))
    render(<Index />, document.getElementById('fleetManagement'));