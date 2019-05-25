import React, { Component } from 'react';
import { render } from 'react-dom';
import Alert from './Components/Alert';
import { SidePanel, SideSection, SidePanelButton } from './Components/SidePanel';
import { BtnClose, BtnClear, BtnInvAll, BtnInvFaxes, Backseat, Boss, Mumble, Status, Type } from './Components/FleetSettings';

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
        this.getFcSettings();

        this.setState({
            fleetId: $("#fleetManagement").data("fleetid")
        }, () => this.getData());

        setInterval(() => this.getData(), 1000 * 10);
    }

    isPublic() {
        return (this.state.fleet) ? this.state.fleet.isPublic : null;
    }

    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/${this.state.fleetId}/data`,
        }).done((result) => {
            this.setState({ fleet: result });
        }).fail((err) => {
            console.error(`React/FleetManagement {FleetManagement@getData} - Error getting fleet information`, err.responseText);
        });
    }

    getFcSettings() {
        $.ajax({
            type: 'get',
            url: `/api/v1/fc-settings`,
            async: false
        }).done((settings) => {
            this.setState({ fcOptions: settings });
        }).fail((err) => {
            console.error(`React/FleetManagement {Index@getFcSettings} - Error getting the FC settings`, err.responseText);
        })
    }

    getFleetSettings() {
        return this.state.fleet || null;
    }

    getSettings() {
        return this.state.fcOptions || null;
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

        return (
            <div className="container">
                {fleetPrivate}
                <SidePanelButton id="fleetSettings" title="Fleet Settings" />
                <SidePanelButton id="fleetCynos" title="Fleet Cynos" />

                <SidePanel id="fleetSettings" title="Fleet Settings">
                    <div className="row">
                        <Boss pilot={(this.state.fleet) ? this.state.fleet.bossPilot : null}
                            pilots={(this.getSettings()) ? this.getSettings().pilots : null}
                            u={this.getData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Backseat account={(this.state.fleet)? this.state.fleet.backseatAccount : null}
                            u={this.getData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Mumble channel={(this.getFleetSettings()) ? this.getFleetSettings().commChannel : null}
                            options={(this.getSettings()) ? this.getSettings().comms : null}
                            u={this.getData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Type type={(this.getFleetSettings()) ? this.getFleetSettings().type : ""}
                            options={(this.getSettings()) ? this.getSettings().fleetTypes : null}
                            u={this.getData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Status public={this.isPublic()}
                            u={this.getData.bind(this)}
                            fleetId={this.state.fleetId} />
                    </div>

                    <hr />

                    <div className="row">
                        <BtnClose fleetId={this.state.fleetId} />
                        <BtnClear fleetId={this.state.fleetId} />

                        <BtnInvAll fleetId={this.state.fleetId} />
                        <BtnInvFaxes fleetId={this.state.fleetId} />
                    </div>
                </SidePanel>
                
                    
                <SidePanel id="fleetCynos" title="Fleet Cynos">

                </SidePanel>
            </div>
        )
    }
}



if (document.getElementById('fleetManagement'))
    render(<Index />, document.getElementById('fleetManagement'));