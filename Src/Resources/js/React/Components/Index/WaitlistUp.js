import React, { Component } from 'react';
import SelectPilot from './JoinWaitlist/SelectPilot';
import SelectShips from './JoinWaitlist/SelectShips';
import SelectRoles from './JoinWaitlist/SelectRoles';
import LeaveWlBtn from './JoinWaitlist/LeaveWlBtn';

export default class WaitlistUp extends Component {
    constructor(props) {
        super(props);

        this.state = {
            selectedPilot: null,
            selectedRoles: [],
            selectedFits: [],
            key: 0
        }
    }

    getRoles() {
        return this.props.options ? this.props.options.roles : null;
    }

    getShips() {
        return this.props.options ? this.props.options.fittings : null;
    }

    updateSelectedPilot(pilot_id) {
        this.setState({ selectedPilot: pilot_id });
    }

    updateSelectedFits(isSelected, fit_id) {
        let fits = this.state.selectedFits;

        if (isSelected) {
            fits.push(fit_id);
        } else {

            for (let i = 0; i < fits.length; i++) {
                if (fits[i] === fit_id)
                    fits.splice(i, 1);
            }
        }

        this.setState({ selectedFits: fits });
    }

    updateSelectedRoles(isSelected, role_id) {
        let roles = this.state.selectedRoles;

        if (isSelected) {
            roles.push(role_id);
        } else {
            for (let i = 0; i < roles.length; i++) {
                if (roles[i] === role_id)
                    roles.splice(i, 1);
            }
        }

        this.setState({ selectedRoles: roles });
    }

    AddPilotToWaitlist() {   
        if (this.state.selectedFits == null || this.state.selectedFits.length == 0) {
            console.error("You must select at least one fit");
            return;
        }

        $.ajax({
            type: 'post',
            url: `${this.props.baseUri}`,
            data: {
                pilot_id: this.state.selectedPilot,
                role_ids: this.state.selectedRoles.join(),
                fit_ids: this.state.selectedFits.join()
            }
        }).done(() => {
            this.props.u();
            this.setState({ key: this.state.key + 1, selectedFits: [], selectedRoles: [] });
        }).fail((err) => {
            console.error(`[React/WaitlistUp] @addPilotToWaitlist - Error joining the waitlist`, err.responseText);
        })  
    }

    render() {
        let disabled = this.state.selectedFits.length > 0 ? false : true;

        return (
            <div>
                <h5 className="pb-4">Join the Waitlist</h5>
                <div className="row pb-4">
                    <div className="col-md-10 col-sm-12">
                        <SelectPilot pilots={this.props.pilots || null}
                            prefPilot={this.props.prefPilot || null}
                            selectedPilot={this.updateSelectedPilot.bind(this)}
                            key={this.state.key} />
                    </div>
                </div>

                <div className="row">
                    <div className="col-lg-6 col-md-12">
                        <SelectShips fits={this.getShips()}
                            selectedFit={this.updateSelectedFits.bind(this)}
                            key={this.state.key} />
                    </div>
                    <div className="col-lg-6 col-md-12">
                        <SelectRoles roles={this.getRoles()}
                            selectedRole={this.updateSelectedRoles.bind(this)}
                            key={this.state.key} />
                    </div>
                </div>

                <button className="btn btn-success" onClick={this.AddPilotToWaitlist.bind(this)} disabled={disabled}>Join the Waitlist <i className="fas fa-user-plus"></i></button>
                <LeaveWlBtn isOnWl={!this.props.isOnWl} u={this.props.u}/>
            </div>    
        )
    }
}